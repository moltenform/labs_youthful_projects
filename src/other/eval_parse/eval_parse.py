# Ben Fisher
# an unorthodox way to parse.


class FunctionSymbol(object):
    def __init__(self, name):
        self.name = name

    def __call__(self, *args):
        return SubExp(self, list(args))

    def __str__(self):
        return 'FN_' + self.name


class VariableSymbol(object):
    def __init__(self, name):
        self.name = name

    def __add__(self, other):
        return SubExp('OP_ADD', [self, other])

    def __mul__(self, other):
        return SubExp('OP_MULT', [self, other])

    def __str__(self):
        return 'VAR_' + self.name


class SubExp(VariableSymbol):
    def __init__(self, op, args):
        self.op = op
        self.args = args

    def __str__(self):
        return str(self.op) + '(' + ','.join(map(str, self.args)) + ')'


def strangeparser(s):
    #parse something by evaluating it as if it were Python code

    symbols = {}
    #create objects for the symbols in the string
    snospace = s.replace(' ', '').replace('\t', '') + ' '
    import re
    for match in re.finditer('[a-zA-Z_]+', snospace):
        strfound = match.group()
        if strfound not in symbols:
            #assume that if the next character is &quot;(&quot;, then it is a function
            if snospace[match.end()] == '(':
                symbols[strfound] = FunctionSymbol(strfound)
            else:
                symbols[strfound] = VariableSymbol(strfound)
    # evaluate it
    try:
        return eval(s, globals(), symbols)
    except Exception as e:
        print('Could not parse. %s' % str(e))
        return None


def main():
    tree = strangeparser('a+b+c')
    print(tree)
    # OP_ADD(OP_ADD(VAR_a,VAR_b),VAR_c)
    tree = strangeparser('f(f( a+f(a * b * f(c))))')
    print(tree)
    # FN_f(FN_f(OP_ADD(VAR_a,FN_f(OP_MULT(OP_MULT(VAR_a,VAR_b),FN_f(VAR_c))))))


main()

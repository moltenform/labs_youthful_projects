
void save(double a,double b)
{
	FILE*fp = fopen("..\\saved.txt", "a"); //append mode
	fprintf(fp, "%f,%f\n", a,b);
	fclose(fp);
}

//http://www.daniweb.com/code/post968823.html
typedef struct node node ;
struct node {  double vala; double valb;  node* next ; };
node * NSaved = NULL;
node* choose_random_node( node* first )
{
  int num_nodes = 0 ; // nodes seen so far
  node* selected = NULL ; // selected node
  node* pn = NULL ;
  for( pn = first ; pn != NULL ; pn = pn->next )
    if(  ( rand() % ++num_nodes  ) == 0 ) selected = pn ;
  return selected ;
}
node* createNode(double a, double b, node *next)
{
node* newnode = (node*) malloc(sizeof(node));
newnode->vala=a; newnode->valb=b;
newnode->next = NULL;
return newnode;
}
void jump(double *outA, double *outB)
{
		//FILE*fdbg = fopen("..\\debug.txt", "a");
	double a,b;
	if (NSaved==NULL)
	{
		//fprintf(fdbg, "mknew\n"); /////////
		NSaved = createNode(-1.1, 1.72, NULL);
		node*cur = NSaved;
		FILE*fp = fopen("..\\saved.txt", "r");
		while (fscanf(fp, "%lf,%lf\n", &a,&b)==2)
		{
			//fprintf(fdbg, "val%lf,%lf\n",a,b); /////////
			cur->next = createNode(a,b, NULL);
			cur = cur->next;
		}
		fclose(fp);
	}
	node * picked = choose_random_node(NSaved);
	*outA = picked->vala;
	*outB = picked->valb;
			//fprintf(fdbg, "PICKED%f,%f\n",*outA, *outB); /////////
}



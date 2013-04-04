
void sample() 
{
	FILE * fout;
	fout = fopen("test.simple","wb");

	fputc('S', fout);
	fputc('2', fout);
	fputc('4', fout);

	int x,y,width,height;

	width = 512; height = 256;
	fwrite(&width, sizeof(int), 1, fout); 
	fwrite(&height, sizeof(int), 1, fout); 

	for (y=0; y<height; y++)
	{
		for (x=0; x<width; x++)
		{
			fputc( y%256 , fout); //Red
			fputc( x%256 , fout); //Green
			fputc( 0 , fout);          //Blue
		}
	}
	fclose(fout);
}

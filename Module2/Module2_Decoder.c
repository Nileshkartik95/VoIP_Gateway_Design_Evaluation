#include<stdio.h>
#include<stdint.h>

#define AUDIO_HEADER (44)
#define	SIGN_BIT	(0x80)		/* Sign bit for a A-law byte. */
#define	QUANT_MASK	(0xf)		/* Quantization field mask. */
#define	NSEGS		(8)		/* Number of A-law segments. */
#define	SEG_SHIFT	(4)		/* Left shift for segment number. */
#define	SEG_MASK	(0x70)		/* Segment field mask. */
#define	BIAS		(0x84)		/* Bias for linear code. */

//@ref: http://www.topherlee.com/software/pcm-tut-wavformat.html
uint8_t wavformat_header[]={
0x52, 0x49, 0x46, 0x46, //RIFF
0x18, 0x82, 0x02, 0x00,    //file size -8
0x57, 0x41, 0x56, 0x45, //WAVE
0x66,0x6d, 0x74,0x20, //FMT
0x10,0x00, 0x00,0x00, //Length of format data
0x01,0x00, // PCM
0x01,0x00, //No of channels 
0x40,0x1f,0x00,0x00, //sampling rate
0x80,0x3E,0x00,0x00, //cumulative data rate
0x02,0x00, //16-bit mono
0x10,0x00, //bits-per-sample
0x64,0x61,0x74,0x61, //data chunk header
0xEC,0x81, 0x02,0x00}; //File size -44


/*
 * Snack_Mulaw2Lin() - Convert a u-law value to 16-bit linear PCM
 *
 * First, a biased linear code is derived from the code word. An unbiased
 * output can then be obtained by subtracting 33 from the biased code.
 *
 * Note that this function expects to be passed the complement of the
 * original code word. This is in keeping with ISDN conventions.
 * @ref: https://opensource.apple.com/source/tcl/tcl-20/tcl_ext/snack/snack/generic/g711.c
 */
short
Snack_Mulaw2Lin(
	unsigned char	u_val)
{
	short		t;

	/* Complement to obtain normal u-law value. */
	u_val = ~u_val;

	/*
	 * Extract and bias the quantization bits. Then
	 * shift up by the segment number and subtract out the bias.
	 */
	t = ((u_val & QUANT_MASK) << 3) + BIAS;
	t <<= ((unsigned)u_val & SEG_MASK) >> SEG_SHIFT;

	return ((u_val & SIGN_BIT) ? (BIAS - t) : (t - BIAS));
}

int main()
{
    uint8_t buffer = 0;
    FILE * input_audio_file = fopen("Au8A_eng_f3.wav", "rb");
    fseek(input_audio_file, 0, SEEK_END);
    int filesize = ftell(input_audio_file);
    fseek(input_audio_file, 0, SEEK_SET);
    printf("Filesize : %d", filesize);
    if (input_audio_file == NULL)
    {
        printf("Error has occurred. Audio file could not be opened\n");
        return -1;
    }

    FILE * output_audio_file = fopen("Output_audio.wav", "wb+");
    if (output_audio_file == NULL)
    {
        printf("Error has occurred. Audio file could not be opened\n");
        return -1;
    }
    int i =0;
    for(; i < AUDIO_HEADER ; i++)
    {
        if(!fwrite(&wavformat_header[i],sizeof(uint8_t),1,output_audio_file))
        {
            printf("Error has occurred. Audio file could not be written\n");
            return -1;
        }
    }
    fseek(input_audio_file,AUDIO_HEADER,SEEK_SET);
    
    uint16_t decoded_buffer;

    while(!feof(input_audio_file))
    {
        fread(&buffer, sizeof(buffer), 1,input_audio_file );
        decoded_buffer = Snack_Mulaw2Lin(buffer); 
        fwrite(&decoded_buffer,sizeof(decoded_buffer),1,output_audio_file);
    }
    fclose(input_audio_file);
    fclose(output_audio_file);
}
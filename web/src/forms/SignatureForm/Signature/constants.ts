import { lightPalette } from '@mms/mms-ui-library/theme';

import facebook from '@/assets/signature/facebook.svg';
import instagram from '@/assets/signature/instagram.svg';
import linkedin from '@/assets/signature/linkedin.svg';
import facebookPng from '@/assets/signature/png/facebook.png';
import instagramPng from '@/assets/signature/png/instagram.png';
import linkedinPng from '@/assets/signature/png/linkedin.png';
import twitterPng from '@/assets/signature/png/twitter.png';
import twitter from '@/assets/signature/twitter.svg';
import { SECONDARY } from '@/theme';

const TEXT_COLOR = lightPalette['text-black'];

export const signatureSizes = {
	s: 20,
	m: 84,
	l: 480,
};

export const logos = [
	{
		svg: instagram,
		png: instagramPng,
		href: 'https://instagram.com/modsencompany/',
		alt: 'instagram logo',
	},
	{
		svg: facebook,
		png: facebookPng,
		href: 'https://facebook.com/ModsenSoftware',
		alt: 'facebook logo',
	},
	{
		svg: linkedin,
		png: linkedinPng,
		href: 'https://linkedin.com/company/modsen/?viewAsMember=true',
		alt: 'linkedin logo',
	},
	{
		svg: twitter,
		png: twitterPng,
		href: 'https://x.com/modsencompany',
		alt: 'x logo',
	},
];

export const HTML_FILE_STYLES = `
  * {
    padding-left: 0;
    padding-right: 0;
    padding-top: 0;
    padding-bottom: 0;
    margin: 0;
    box-sizing: border-box;
    font-family: Onest, sans-serif;
  }
    
  table {
    text-wrap: nowrap;
    border-spacing: 0;
    width: 480px;
    border-collapse: collapse
  }

  table td, table th {
    padding: 0; 
  }

  .signature-table {
    white-space: nowrap;
		width: 480px;
		user-select: none;
		margin: 0;
		border: 0;
		borderSpacing: 0;
  }

  .signature-row {
    width: 100%;
    height: 20px;
    line-height: 100%;
    margin: 0;
    padding: 0;
  }

  .signature-row > td {
    height: 20px;
  }
 
  .signature-fullname-wrapper {
    width: 320px;
    height: 22px;
    line-height: 100%;
    display: flex;
    align-items: center;
  }

  .signature-fullname {
    color: ${TEXT_COLOR};
    font-style: normal;
    font-size: 16px;
    font-weight: 700;
    line-height: 24px;
    letter-spacing: 0.32px;
  }

  .signature-logo-wrapper {
    width: auto;
    text-align: end;
    height: 20px;
    line-height: 100%;
  }

  .signature-position {
    color: ${SECONDARY} !important;
    padding: 0;
    margin: 0;
    font-size: 14px;
    font-weight: 600;
    line-height: 18px;
    margin-bottom: 8px;
    display: inline-block;
    text-decoration: none;
  }

  .signature-link-wrapper {
    height: 14px;
    line-height: 14px;
    font-size: 14px;
    display: inline-flex;
    justify-content: center;
    width: fit-content;
  }
  
  .signature-link-wrapper:not(:first-child) {
    justify-content: center;
  }

  .signature-link-phone{
    text-Decoration: none;
	  color: ${TEXT_COLOR};
	  font-weight: normal;
	  position: relative;
	  display: inline-block;
  }


  .signature-link-website {
    text-Decoration: none;
	  color: ${TEXT_COLOR};
	  font-weight: normal;
	  position: relative;
  }

  .signature-phone {
    width: 150px;
  }

  .signature-website {
    width: 256px;
  }

  .signature-media-link-wrapper {
    display: flex;
	  justify-content: flex-end;
    flexWrap: 'nowrap'
  }
 
  .signature-media-link {
    padding: 0;
    display: flex;
    justify-content: center;
    align-items: center;
  }
  
`;

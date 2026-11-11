import styled from '@emotion/styled';

import currencySymbolFontUrl from '@/assets/fonts/nbrb.woff2';

import { CURRENCY_SYMBOL_FONT_FAMILY } from './constants';

export const CurrencySymbolText = styled('span')<{ color?: string }>(
	({ color }) => ({
		display: 'inline',
		color,
	})
);

export const BYNCurrencySymbol = styled(CurrencySymbolText)(({ theme }) => ({
	'@font-face': {
		fontFamily: CURRENCY_SYMBOL_FONT_FAMILY,
		src: `url(${currencySymbolFontUrl}) format('woff2')`,
		fontWeight: 'normal',
		fontStyle: 'normal',
		fontDisplay: 'block',
	},
	fontFamily: `"${CURRENCY_SYMBOL_FONT_FAMILY}", sans-serif`,
	fontWeight: theme.typography.body['s-400'].fontWeight,
}));

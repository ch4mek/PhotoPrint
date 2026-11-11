import styled from '@emotion/styled';
import { lightPalette } from '@mms/mms-ui-library/theme';

import { signatureSizes } from './constants';

export const TableSignature = styled('table')({
	whiteSpace: 'nowrap',
	minHeight: signatureSizes.m,
	height: 'auto',
	width: '100%',
	maxWidth: signatureSizes.l,
	textWrap: 'nowrap',
	background: lightPalette['background-white'],
	backgroundColor: lightPalette['background-white'],
	padding: 0,
	userSelect: 'none',
	margin: 0,
	borderSpacing: 0,
	td: {
		padding: 0,
	},
});

export const TableSignatureInner = styled('table')({
	height: 'auto',
	width: '100%',
});

export const TableRowSignature = styled('tr')({
	fontFamily: 'Inter, sans-serif',
	width: '100%',
	height: signatureSizes.s,
	lineHeight: '100%',
	margin: 0,
	padding: 0,
});

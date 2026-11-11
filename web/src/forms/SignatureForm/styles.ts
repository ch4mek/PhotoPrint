import styled from '@emotion/styled';
import { lightPalette } from '@mms/mms-ui-library/theme';

import { BorderRadiuses } from '@/constants/borderRadiuses';

import { SIGNATURE_WRAPPER_MAX_WIDTH } from './constants';

export const SignatureWrapper = styled('div')(({ theme }) => ({
	maxWidth: SIGNATURE_WRAPPER_MAX_WIDTH,
	minWidth: 'fit-content',
	padding: theme.spaces.l,
	borderRadius: BorderRadiuses.m,
	border: `1px solid ${theme.palette['graphics-divider-1']}`,
	background: lightPalette['background-white'],
}));

export const SignatureFieldsWrapper = styled('div')(({ theme }) => ({
	width: '100%',
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.l,
}));

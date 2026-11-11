import styled from '@emotion/styled';

import { CERTIFICATE_IMAGE_SIZE } from '../../constants';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { ThemeMode, WHITE } from '@/theme';

export const SpecializationCellWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	alignItems: 'center',
	padding: `${theme.spaces.xs}px 0`,
	gap: theme.spaces.s,
	height: '100%',
	width: '100%',

	'& > div': {
		height: '100%',
	},
}));

export const CertificateImage = styled('img')(({ theme }) => ({
	height: CERTIFICATE_IMAGE_SIZE,
	width: 'auto',

	...(theme.palette.mode === ThemeMode.DARK && {
		backgroundColor: WHITE,
		borderRadius: BorderRadiuses.half,
	}),
}));

import styled from '@emotion/styled';

import { BorderRadiuses } from '@/constants/borderRadiuses';

export const UploadWrapper = styled('div')<{
	uploadRadius: number | BorderRadiuses;
	size: number;
}>(({ theme, size, uploadRadius }) => ({
	backgroundColor: theme.palette['background-base-2'],
	width: size,
	height: size,
	position: 'relative',
	border: `1px solid ${theme.palette['graphics-divider-2']}`,
	borderRadius: uploadRadius || BorderRadiuses.m,

	'& > div': {
		width: '100%',
		height: '100%',
	},
}));

export const ImageWrapper = styled('img')<{
	uploadRadius: number | BorderRadiuses;
}>(({ uploadRadius }) => ({
	width: '100%',
	height: '100%',

	objectFit: 'cover',

	borderRadius: uploadRadius || BorderRadiuses.m,
}));

export const NoImageWrapper = styled('div')<{ disabled?: boolean }>(
	({ theme, disabled }) => ({
		display: 'flex',
		justifyContent: 'center',
		alignItems: 'center',

		width: '100%',
		height: '100%',

		cursor: disabled ? 'auto' : 'pointer',
		color: theme.palette['graphics-light-gray'],

		'& > svg path': {
			vectorEffect: 'non-scaling-stroke',
		},
	})
);

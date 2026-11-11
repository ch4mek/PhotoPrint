import styled from '@emotion/styled';
import { Avatar } from '@mms/mms-ui-library/components/Avatar';

import { addOpacityToColor } from '@/helpers/addOpacityToColor';
import { BLACK, WHITE } from '@/theme';

import { AVATAR_OVERLAY_Z_INDEX, AVATAR_SIZE } from './constants';

export const AvatarImage = styled(Avatar)(({ theme }) => ({
	fontWeight: 700,
	fontSize: theme.typography.pxToRem(16),
	lineHeight: theme.typography.pxToRem(24),
	width: `${AVATAR_SIZE}px`,
	height: `${AVATAR_SIZE}px`,
	minWidth: `${AVATAR_SIZE}px`,

	'& img': {
		width: `${AVATAR_SIZE}px`,
		height: `${AVATAR_SIZE}px`,
	},
}));

export const AvatarOverlay = styled('div')({
	position: 'absolute',
	inset: 0,
	backgroundColor: addOpacityToColor(BLACK, 0.5),
	display: 'flex',
	alignItems: 'center',
	justifyContent: 'center',
	opacity: 0,
	transition: 'opacity 0.2s ease-in-out',
	zIndex: AVATAR_OVERLAY_Z_INDEX,
	color: WHITE,
	userSelect: 'none',

	'&:hover': {
		opacity: 1,
		cursor: 'pointer',
	},
});

export const AvatarContainer = styled('div')`
	position: relative;
	width: ${AVATAR_SIZE}px;
	height: ${AVATAR_SIZE}px;
	border-radius: 50%;
	overflow: hidden;
`;

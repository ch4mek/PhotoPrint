import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { VIEWER_HEADER_HEIGHT } from '../../constants';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { BLACK, WHITE } from '@/theme';

const VIEWER_HEADER_WRAPPER_BACKGROUND = `${BLACK}CC`;
const BUTTON_HOVERED_BACKGROUND = `${WHITE}1A`;
const BUTTON_SIZE = 36;
const FILE_ICON_SIZE = {
	width: 17,
	height: 20,
};

export const ViewerHeaderWrapper = styled('div')(({ theme }) => ({
	padding: `${theme.spaces.s}px ${theme.spaces.m}px`,
	backgroundColor: VIEWER_HEADER_WRAPPER_BACKGROUND,
	display: 'flex',
	alignItems: 'center',
	justifyContent: 'space-between',
	height: VIEWER_HEADER_HEIGHT,
}));

export const FileNameBadge = styled('div')(({ theme }) => ({
	display: 'flex',
	alignItems: 'center',
	gap: theme.spaces.s,
}));

export const FileNameTypography = styled(Typography)(({ theme }) => ({
	color: theme.palette['text-white'],
}));

export const FileIconWrapper = styled('div')({
	display: 'flex',
	alignItems: 'center',
	justifyContent: 'center',

	'& svg': {
		...FILE_ICON_SIZE,
	},
});

export const ViewerHeaderButton = styled('button')(({ theme }) => ({
	all: 'unset',
	width: BUTTON_SIZE,
	height: BUTTON_SIZE,
	color: theme.palette['graphics-primary-2'],
	borderRadius: BorderRadiuses.m,
	border: `1px solid ${theme.palette['graphics-input-stroke']}`,
	backgroundColor: theme.palette['background-white'],
	display: 'flex',
	justifyContent: 'center',
	alignItems: 'center',

	':hover': {
		cursor: 'pointer',
		backgroundColor: BUTTON_HOVERED_BACKGROUND,
		borderColor: 'transparent',
		color: WHITE,
	},
}));

import { Theme } from '@emotion/react';
import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { TOOLBAR_BUTTON_CLASSNAME } from '@/components/PlateToolbars/constants';
import { BorderRadiuses } from '@/constants/borderRadiuses';
import { IconSize } from '@/constants/iconSizes';
import { popUpBreakpoints } from '@/constants/popUp';

const VERTICAL_ROW_HEIGHT = 56;
const VERTICAL_ROW_SMALL_HEIGHT = 24;
const VERTICAL_ROW_WIDTH = 1;
const POPOVER_WIDTH = 106;

export const GroupButtonsWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	alignItems: 'center',
	gap: theme.spaces.m,
	color: theme.palette['text-black'],
}));

export const ChevronButtonWrapper = styled('div')({
	display: 'flex',
	flexDirection: 'row',
});

export const ButtonsWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexWrap: 'wrap',
	gap: theme.spaces.s,
	color: theme.palette['text-black'],

	width: '100%',

	[`& .${TOOLBAR_BUTTON_CLASSNAME}`]: {
		height: IconSize.M,
		width: IconSize.M,

		'& > svg': {
			height: IconSize.M,
			width: IconSize.M,
		},
	},
}));

export const GroupTitle = styled(Typography)(({ theme }) => ({
	color: theme.palette['text-gray'],
	marginTop: 'auto',
}));

export const CustomVerticalRow = styled('div')(({ theme }) => ({
	width: `${VERTICAL_ROW_WIDTH}px`,
	height: `${VERTICAL_ROW_HEIGHT}px`,
	backgroundColor: theme.palette['graphics-divider-1'],

	[theme.breakpoints.down(popUpBreakpoints.tabletXL)]: {
		height: `${VERTICAL_ROW_SMALL_HEIGHT}px`,
	},
}));

export const getToolbarPopoverStyles = (theme: Theme) => ({
	background: theme.palette['background-white'],
	borderRadius: BorderRadiuses.m,
	border: `1px solid ${theme.palette['graphics-input-stroke']}`,
	boxShadow: theme.namedShadows.dropDownListWrapper,
	padding: theme.spaces.s,
	width: POPOVER_WIDTH,
});

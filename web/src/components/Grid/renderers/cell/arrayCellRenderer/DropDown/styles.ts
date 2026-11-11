import styled from '@emotion/styled';
import { Popover } from '@mms/mms-ui-library/components/Popover';
import { getScrollbarStyles } from '@mms/mms-ui-library/components/Scrollbar';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { BorderRadiuses } from '@/constants/borderRadiuses';

const dropDownListSize = {
	height: 300,
	width: 200,
};

export const DropDownList = styled(Popover)(({ theme }) => ({
	position: 'absolute',
	maxHeight: dropDownListSize.height,
	maxWidth: dropDownListSize.width,
	width: '100%',

	padding: `${theme.spaces.s}px`,
	margin: 0,

	backgroundColor: theme.palette['background-white'],
	border: `1px solid ${theme.palette['graphics-input-stroke']}`,
	borderRadius: BorderRadiuses.m,
	fontSize: 'inherit',
	boxShadow: theme.namedShadows.dropDownListWrapper,

	...getScrollbarStyles(theme, 4, 'vertical'),
}));

export const DropDownItemWrapper = styled.div(({ theme }) => ({
	padding: `${theme.spaces.s}px`,

	'&:hover': {
		backgroundColor: theme.palette['hover-base-2'],
		borderRadius: BorderRadiuses.m,
	},
}));

export const DropDownItem = styled(Typography)(({ theme }) => ({
	display: 'block',
	width: '100%',
	textOverflow: 'ellipsis',
	overflow: 'hidden',
	color: theme.palette.text.primary,
}));

import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { popUpBreakpoints } from '@/constants/popUp';

const POPUP_TITLE_WIDTH = 103;

const COLUMN_WRAPPER_TABLET_XS_GRID_TEMPLATE_COLUMNS =
	'minMax(144px, 1fr) minMax(297px, 350px)';

export const ColumnWrapper = styled('div')(({ theme }) => ({
	display: 'grid',
	gap: theme.spaces.l,

	[theme.breakpoints.up(popUpBreakpoints.tabletXS)]: {
		gridTemplateColumns: COLUMN_WRAPPER_TABLET_XS_GRID_TEMPLATE_COLUMNS,
		gap: theme.spaces.l,
	},
}));

export const ColorPickerFieldWrapper = styled('div')(({ theme }) => ({
	marginTop: theme.spaces.l,

	[theme.breakpoints.up(popUpBreakpoints.tabletXS)]: {
		marginTop: 'auto',
	},
}));

export const StyledLeftSideBox = styled('div')({
	display: 'flex',
	flexDirection: 'column',
});

export const StyledRightSideBox = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	justifyContent: 'space-between',
	gap: theme.spaces.l,
}));

export const TabWrapper = styled('div')<{ height: string }>(({ height }) => ({
	display: 'flex',
	flexDirection: 'column',
	height,
	minHeight: 0,
}));

export const UserNameContainer = styled(Typography)({
	maxWidth: `calc(100% - ${POPUP_TITLE_WIDTH}px)`,
	width: '100%',
	display: 'flex',
	flexWrap: 'nowrap',
});

export const UserName = styled('span')({
	overflow: 'hidden',
	textOverflow: 'ellipsis',
	textWrap: 'nowrap',
	maxWidth: '100%',
});

import styled from '@emotion/styled';

import { IconSize } from '@/constants/iconSizes';
import { DEFAULT_OPACITY } from '@/constants/opacities';

import { getHeaderCellContentJustifyContent } from './utils/getHeaderCellContentJustifyContent';

export const HeaderCellBaseContentWrapper = styled('div')<{
	isColumnSorted: boolean;
}>(({ isColumnSorted }) => ({
	display: 'flex',
	justifyContent: 'left',
	position: 'relative',
	alignItems: 'center',
	minWidth: 0,

	button: {
		opacity: isColumnSorted ? DEFAULT_OPACITY : 0,
		width: IconSize.S,
		height: IconSize.S,
	},

	svg: {
		width: IconSize.S,
		height: IconSize.S,
	},
}));

export const HeaderCellContentWrapper = styled('div')<{
	isHasSortableIcon: boolean;
	isContentAlignedToEnd?: boolean;
}>(({ theme, isHasSortableIcon, isContentAlignedToEnd }) => ({
	display: 'flex',
	justifyContent: getHeaderCellContentJustifyContent({
		isContentAlignedToEnd,
		isHasSortableIcon,
	}),
	position: 'relative',
	alignItems: 'center',
	width: '100%',
	minWidth: 0,
	gap: theme.spaces.xs,
}));

export const getInfoTooltipCircleStyles = () => ({
	width: `${IconSize.S}px`,
	height: `${IconSize.S}px !important`,
	justifyContent: 'center',
});

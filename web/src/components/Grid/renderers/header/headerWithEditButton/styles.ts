import styled from '@emotion/styled';

import {
	DEFAULT_DISABLED_GRID_CELL_OPACITY,
	DEFAULT_OPACITY,
} from '@/constants/opacities';

export const HeaderCellContentWrapper = styled('div')<{
	isMarkAsPast: boolean;
}>(({ isMarkAsPast }) => ({
	display: 'flex',
	justifyContent: 'space-between',
	position: 'relative',
	alignItems: 'center',
	width: '100%',
	opacity: isMarkAsPast ? DEFAULT_DISABLED_GRID_CELL_OPACITY : DEFAULT_OPACITY,
}));

export const EditIconWrapper = styled('div')({
	display: 'flex',
	alignItems: 'center',
	justifyContent: 'center',
});

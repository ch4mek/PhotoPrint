import styled from '@emotion/styled';

import { DEFAULT_OPACITY, HALF_OPACITY } from '@/constants/opacities';
import { WithDisabled } from '@/types/helpers';

export const TotalCellWrapper = styled('div')<WithDisabled>(({ disabled }) => ({
	display: 'flex',
	alignItems: 'center',
	justifyContent: 'center',
	width: '100%',
	height: '100%',
	opacity: disabled ? HALF_OPACITY : DEFAULT_OPACITY,
}));

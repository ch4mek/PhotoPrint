import styled from '@emotion/styled';

import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import { MIN_ABSENCE_REQUESTS_HEIGHT } from '@/constants/absence';

export const GridWrapper = styled(DataGridHoverRowWrapper)(({ theme }) => ({
	minHeight: MIN_ABSENCE_REQUESTS_HEIGHT,
	maxHeight: MIN_ABSENCE_REQUESTS_HEIGHT,
	marginBottom: theme.spaces.l,
}));

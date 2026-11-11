import styled from '@emotion/styled';

import { AUTOSELECT_FILTER_WIDTH } from '@/constants/commonStyles';

export const AutoselectWrapper = styled('div')(({ theme }) => ({
	width: AUTOSELECT_FILTER_WIDTH,
	background: theme.palette['background-white'],
}));

export const FiltersWrapper = styled('div')(({ theme }) => ({
	paddingBlock: theme.spaces.s,
	display: 'flex',
	flexDirection: 'row',
	alignItems: 'center',
	gap: theme.spaces.s,
}));

import styled from '@emotion/styled';

import { DATE_VALUE_FILTER_WIDTH } from './constants';

export const DateValueFilterControlsWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	alignItems: 'center',
	gap: theme.spaces.s,
}));

export const DateValueFilterWrapper = styled('div')({
	width: DATE_VALUE_FILTER_WIDTH,
});

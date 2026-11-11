import styled from '@emotion/styled';

import { TotalTimeChartLabelWrapperProps } from '@/components/TotalTimeChartLabel/types';

export const TotalTimeChartLabelWrapper = styled(
	'div'
)<TotalTimeChartLabelWrapperProps>(({ theme, isLabelStatic }) => ({
	display: 'flex',
	gap: theme.spaces.xs,
	position: 'absolute',
	top: theme.spaces.m,
	left: theme.spaces.m,

	...(isLabelStatic && {
		position: 'static',
	}),
}));

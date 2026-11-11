import styled from '@emotion/styled';

import { WidgetCarousel } from '../../../WidgetCarousel';

export const DevelopmentPlansListWrapper = styled('ul')(({ theme }) => ({
	margin: 0,
	padding: 0,
	listStyle: 'none',

	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.m,

	width: '100%',
}));

export const DevelopmentPlansWidgetCarouselWrapper = styled(WidgetCarousel)(
	({ theme }) => ({
		gap: theme.spaces.m,
	})
);

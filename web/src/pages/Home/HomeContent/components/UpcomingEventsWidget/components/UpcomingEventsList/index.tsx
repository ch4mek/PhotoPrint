import { useHasScreenWidth } from '@mms/mms-ui-library/hooks/useHasScreenWidth';
import { useMemo } from 'react';

import { UpcomingEventsWidgetProps } from '../../types';
import { UpcomingEventItem } from '../UpcomingEventItem';

import { WidgetCarousel } from '@/pages/Home/HomeContent/components/WidgetCarousel';
import { HOME_MOBILE_BREAKPOINT } from '@/pages/Home/HomeContent/constants';

import { UpcomingEventsListWrapper } from './styles';

export function UpcomingEventsList({ data }: UpcomingEventsWidgetProps) {
	const isDesktopView = useHasScreenWidth(HOME_MOBILE_BREAKPOINT);

	const itemsRender = useMemo(
		() =>
			data.map((item, index) => (
				<UpcomingEventItem key={index} {...{ item, isDesktopView }} />
			)),
		[data, isDesktopView]
	);

	return isDesktopView ? (
		<UpcomingEventsListWrapper>{itemsRender}</UpcomingEventsListWrapper>
	) : (
		<WidgetCarousel>{itemsRender}</WidgetCarousel>
	);
}

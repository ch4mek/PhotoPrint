import { useHasScreenWidth } from '@mms/mms-ui-library/hooks/useHasScreenWidth';
import { EmptyEventsDarkIcon } from '@mms/mms-ui-library/icons/base/EmptyEventsDarkIcon';
import { EmptyEventsLightIcon } from '@mms/mms-ui-library/icons/base/EmptyEventsLightIcon';
import { useTranslation } from 'react-i18next';

import { HOME_MOBILE_BREAKPOINT } from '../../constants';
import { EmptyResult } from '../EmptyResult';

import { Widget } from '@/components/Widget';

import { UpcomingEventsList } from './components/UpcomingEventsList';
import { UpcomingEventsWidgetProps } from './types';

const UPCOMING_EVENT_WIDGET_HEIGHT_MOBILE = 180;

export function UpcomingEventsWidget({ data }: UpcomingEventsWidgetProps) {
	const { t } = useTranslation('home', { keyPrefix: 'widgets.upcomingEvents' });
	const isDesktopView = useHasScreenWidth(HOME_MOBILE_BREAKPOINT);

	return (
		<Widget
			{...{ title: t('title') }}
			height={isDesktopView ? undefined : UPCOMING_EVENT_WIDGET_HEIGHT_MOBILE}
		>
			{data.length === 0 ? (
				<EmptyResult
					darkThemeIcon={<EmptyEventsDarkIcon />}
					lightThemeIcon={<EmptyEventsLightIcon />}
					title={t('noEvents')}
				/>
			) : (
				<UpcomingEventsList {...{ data }} />
			)}
		</Widget>
	);
}

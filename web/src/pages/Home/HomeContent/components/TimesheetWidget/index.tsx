import { useTheme } from '@emotion/react';
import { ContentCalendar } from '@mms/mms-ui-library/components/ContentCalendar';
import { useHasScreenWidth } from '@mms/mms-ui-library/hooks/useHasScreenWidth';
import { memo } from 'react';
import { useTranslation } from 'react-i18next';

import { HOME_MOBILE_BREAKPOINT } from '../../constants';

import { Loader } from '@/components/Loader';
import { Widget } from '@/components/Widget';
import { Routes } from '@/constants/routes';
import { useCustomNavigate } from '@/hooks/useCustomNavigate';

import { TimeshitsWidgetProps } from './types';
import { convertDataForCalendar } from './utils/convertDataForCalendar';

function TimesheetWidgetInner({ data, isLoading }: TimeshitsWidgetProps) {
	const theme = useTheme();
	const { t } = useTranslation('home', { keyPrefix: 'widgets.timesheet' });
	const navigate = useCustomNavigate();
	const daysData = convertDataForCalendar(data, theme);
	const isDesktopView = useHasScreenWidth(HOME_MOBILE_BREAKPOINT);

	const handleRedirectToTimesheet = () => {
		navigate({
			to: `${Routes.timeSheet}`,
		});
	};

	return (
		<Widget
			{...{ title: t('title') }}
			height={isDesktopView ? undefined : 'auto'}
			handleClick={handleRedirectToTimesheet}
		>
			<Loader size="small" isLoading={isLoading}>
				<ContentCalendar daysData={daysData || []} />
			</Loader>
		</Widget>
	);
}

export const TimesheetWidget = memo(TimesheetWidgetInner);

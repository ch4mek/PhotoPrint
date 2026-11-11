import { useTheme } from '@emotion/react';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import i18next from 'i18next';

import { getDateLocale } from '@/i18n/locale';

import { TimelineCurrentDateWrapper } from './styles';

export function CurrentDate({ selectedDate }: { selectedDate: Date }) {
	const theme = useTheme();
	const locale = getDateLocale(i18next.language);

	return (
		<TimelineCurrentDateWrapper>
			<Typography color={theme.palette['text-accent']} variant="m-700">
				{selectedDate.getDate().toString().padStart(2, '0')}
			</Typography>
			<Typography color={theme.palette['text-accent']} variant="m-400">
				{selectedDate.toLocaleDateString(locale, { weekday: 'long' })}
			</Typography>
		</TimelineCurrentDateWrapper>
	);
}

import { useTheme } from '@emotion/react';
import { FormattedTime } from '@mms/mms-ui-library/components/FomattedTime';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { DEFAULT_CHART_LABEL_TITLE } from '@/components/TotalTimeChartLabel/constants';

import { TotalTimeChartLabelWrapper } from './styles';
import type { TotalTimeChartLabelProps } from './types';

export function TotalTimeChartLabel({
	isLabelStatic = false,
	text,
	title = DEFAULT_CHART_LABEL_TITLE,
}: TotalTimeChartLabelProps) {
	const theme = useTheme();
	const [hours, minutes] = text.split(':');

	return (
		<TotalTimeChartLabelWrapper isLabelStatic={!!isLabelStatic}>
			<Typography variant="m-600" color={theme.palette['text-black']}>
				{title}
			</Typography>
			<FormattedTime hours={hours} minutes={minutes} />
		</TotalTimeChartLabelWrapper>
	);
}

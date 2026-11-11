import { TotalTimeChartLabel } from '@/components/TotalTimeChartLabel';
import { REPORTS_CHART_LABEL_TITLE } from '@/pages/Reports/components/ReportsStatistic/constants';
import { ReportsStatisticWrapper } from '@/pages/Reports/components/ReportsStatistic/styles';

export function ReportsStatistic({ totalTime }: { totalTime: string }) {
	return (
		<ReportsStatisticWrapper>
			<TotalTimeChartLabel
				text={totalTime}
				title={REPORTS_CHART_LABEL_TITLE}
				isLabelStatic
			/>
		</ReportsStatisticWrapper>
	);
}

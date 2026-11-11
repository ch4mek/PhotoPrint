import { Loader } from '../Loader';

import { ChartContent } from './ChartContent';
import { HEIGHT_OF_CHART } from './constants';
import { InnerBugFixBox, OuterBugFixBox } from './styles';
import type { ChartProps } from './types';

export function Chart({ data, showLoader, xAxisLabel }: ChartProps) {
	return (
		<Loader
			isLoading={showLoader ?? false}
			size="small"
			height={HEIGHT_OF_CHART}
		>
			<OuterBugFixBox>
				<InnerBugFixBox>
					<ChartContent data={data} xAxisLabel={xAxisLabel} />
				</InnerBugFixBox>
			</OuterBugFixBox>
		</Loader>
	);
}

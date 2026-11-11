import { useMemo } from 'react';
import { RenderCellProps } from 'react-data-grid';

import { PerformanceReviewGridRow } from '../../types';

import { DateTag } from '@/components/DateTag';
import { PERFORMANCE_REVIEW_DATE_TAG_WIDTH } from '@/constants/performanceReview';
import { HandleTagChoose } from '@/pages/PerformanceReview/components/PerformanceReviewGrid/components/datesRenderCell/types';

import { Wrapper } from './styles';

export function DatesRenderCell<T extends PerformanceReviewGridRow>(
	props: RenderCellProps<T>,
	handleTagChoose?: HandleTagChoose
) {
	const {
		row: { id, fullName, dates },
	} = props;

	const userData = useMemo(
		() => ({
			id,
			fullName,
		}),
		[id, fullName]
	);

	return dates.map(
		({
			date,
			performanceReviewDateType,
			conversationId,
			performanceReviewId,
		}) => (
			<Wrapper key={date}>
				<DateTag
					onClick={handleTagChoose?.(
						userData,
						date,
						performanceReviewId,
						conversationId
					)}
					date={date}
					type={performanceReviewDateType!}
					width={PERFORMANCE_REVIEW_DATE_TAG_WIDTH}
				/>
			</Wrapper>
		)
	);
}

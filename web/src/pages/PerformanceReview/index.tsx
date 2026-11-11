import { PageWrapper } from '@/pages/PerformanceReview/styles';

import { PerformanceReviewGrid } from './components/PerformanceReviewGrid';
import { PerformanceReviewHeader } from './components/PerformanceReviewHeader';

export function PerformanceReviewPage() {
	return (
		<PageWrapper>
			<PerformanceReviewHeader />
			<PerformanceReviewGrid />
		</PageWrapper>
	);
}

import React from 'react';

import { PageHeader } from '@/components/PageHeader';
import { PageTitles } from '@/constants/pageTitles';

import { PerformanceReviewControls } from './components/PerformanceReviewControls';
import { PerformanceReviewHeaderWrapper } from './styles';

export function PerformanceReviewHeader() {
	return (
		<PerformanceReviewHeaderWrapper>
			<PageHeader title={PageTitles.PerformanceReview} />
			<PerformanceReviewControls />
		</PerformanceReviewHeaderWrapper>
	);
}

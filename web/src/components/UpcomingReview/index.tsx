import { Typography } from '@mms/mms-ui-library/components/Typography';
import { useHasScreenWidth } from '@mms/mms-ui-library/hooks/useHasScreenWidth';
import { breakpoints } from '@mms/mms-ui-library/theme';
import { useTranslation } from 'react-i18next';

import { DateTag } from '@/components/DateTag';

import { UpcomingReviewWrapper } from './styles';
import { UpcomingReviewProps } from './types';

const { tabletXS } = breakpoints;

export function UpcomingReview({
	isFetching,
	isLoading,
	upcomingPerfomanceReview,
	performanceReviewDateStatus,
}: UpcomingReviewProps) {
	const isTabletXSWidth = useHasScreenWidth(tabletXS);

	const { t } = useTranslation('home');

	const isShowPerfomanceReview = !isLoading || !isFetching;

	return (
		<UpcomingReviewWrapper>
			{isShowPerfomanceReview && (
				<>
					<Typography variant="m-400">
						{isTabletXSWidth ? t('reviewTitle') : t('shortReviewTitle')}
					</Typography>
					<DateTag
						date={upcomingPerfomanceReview}
						type={performanceReviewDateStatus}
					/>
				</>
			)}
		</UpcomingReviewWrapper>
	);
}

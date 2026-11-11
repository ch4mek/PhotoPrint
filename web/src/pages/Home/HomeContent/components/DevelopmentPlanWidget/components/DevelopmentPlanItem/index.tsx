import { useTheme } from '@emotion/react';
import { Button } from '@mms/mms-ui-library/components/Button';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { useCallback } from 'react';
import { useTranslation } from 'react-i18next';

import { Tag } from '@/components/Tag';
import { DEFAULT_TAG_WIDTH } from '@/constants/commonStyles';
import { developmentPlanStatus } from '@/constants/developmentPlan';
import { formatDateString } from '@/helpers/dateToISOString';
import { getDevelopmentTagColor } from '@/helpers/getDevelopmentStatusTagColor';
import { DevelopmentPlanStatus } from '@/types/developmentPlan';

import {
	ControlWrapper,
	DevelopmentPlanItemWrapper,
	PlanItem,
	PlanSummaryWrapper,
	PlanTypography,
} from './styles';
import { DevelopmentPlanItemProps } from './types';

export function DevelopmentPlanItem({
	item,
	handleSendToReview,
	isDesktopView = true,
}: DevelopmentPlanItemProps) {
	const theme = useTheme();
	const { status, deadlineDate, summary, id } = item;
	const { t } = useTranslation('home', {
		keyPrefix: 'widgets.developmentPlan',
	});
	const { t: tCommon } = useTranslation('common', {
		keyPrefix: 'developmentPlanStatus',
	});
	const statusTitle = developmentPlanStatus[status];

	const { title, ...tagStyles } =
		status === DevelopmentPlanStatus.None
			? {
					title: t('noStatusTagTitle'),
					color: theme.palette['text-black'],
			  }
			: {
					title: statusTitle,
					...getDevelopmentTagColor(statusTitle),
			  };

	const handleSendToReviewClick = useCallback(() => {
		handleSendToReview(id);
	}, [id, handleSendToReview]);

	return (
		<DevelopmentPlanItemWrapper {...{ isDesktopView }}>
			<PlanItem>
				<Tag width={DEFAULT_TAG_WIDTH} {...tagStyles}>
					{tCommon(statusTitle)}
				</Tag>
				<Tooltip
					useIfTextCut
					text={summary}
					textCutOrientation="vertical"
					position="bottom"
				>
					<PlanSummaryWrapper>
						<PlanTypography variant="m-400">{summary}</PlanTypography>
					</PlanSummaryWrapper>
				</Tooltip>
				<ControlWrapper>
					<Typography variant="s-400">
						{formatDateString(deadlineDate, '')}
					</Typography>
					<Button
						view="filled"
						color="secondary"
						isFullWidth={false}
						disabled={statusTitle === developmentPlanStatus.UnderReview}
						onClick={handleSendToReviewClick}
					>
						{t('markAsDoneButtonTitle')}
					</Button>
				</ControlWrapper>
			</PlanItem>
		</DevelopmentPlanItemWrapper>
	);
}

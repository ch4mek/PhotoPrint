import { useTheme } from '@emotion/react';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { useTranslation } from 'react-i18next';

import { CircleColor } from '@/components/CircleColor';
import { INTERVIEW_STATUS_COLORS } from '@/pages/Staffing/components/InterviewTab/constants';
import { ThemeMode } from '@/theme';
import { InterviewStatusNameColorKey } from '@/types/interview';

import { InfoItem, InfoTooltipWrapper } from './styles';
import { InterviewStatusColorGroup } from './types';

export function InfoInterviewStatus() {
	const theme = useTheme();
	const isLightTheme = theme.mode === ThemeMode.LIGHT;
	const { t } = useTranslation('dashboard', { keyPrefix: 'interview' });
	const statusColorGroups = Object.entries(INTERVIEW_STATUS_COLORS).reduce<
		Partial<Record<InterviewStatusNameColorKey, InterviewStatusColorGroup>>
	>((groups, [status, { light, dark, nameColor }]) => {
		const existingGroup = groups[nameColor];

		if (existingGroup) {
			existingGroup.statuses.push(status as InterviewStatusNameColorKey);

			return groups;
		}

		return {
			...groups,
			[nameColor]: {
				light,
				dark,
				statuses: [status as InterviewStatusNameColorKey],
			},
		};
	}, {});

	return (
		<InfoTooltipWrapper>
			{Object.entries(statusColorGroups).map(
				([nameColor, { light, dark, statuses }]) => (
					<InfoItem key={nameColor}>
						<CircleColor color={isLightTheme ? light : dark} />
						<Typography variant="s-400">
							<Typography variant="s-600">
								{t(`tagNames.${nameColor as InterviewStatusNameColorKey}`)}
							</Typography>
							:{' '}
							{statuses
								.map((status) => t(`tagDescriptions.${status}`))
								.join(', ')}
						</Typography>
					</InfoItem>
				)
			)}
		</InfoTooltipWrapper>
	);
}

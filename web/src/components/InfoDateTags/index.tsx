import { Typography } from '@mms/mms-ui-library/components/Typography';
import { useTranslation } from 'react-i18next';

import { tagColors } from '../DateTag/constants';

import { CircleColor } from '@/components/CircleColor';

import { InfoItem } from './styles';

export function InfoDateTags() {
	const { t } = useTranslation('profile', { keyPrefix: 'dateTag' });
	const allTags = Object.values(tagColors);

	return (
		<div>
			{allTags.map(({ nameColor, descriptionColor, backgroundColor }) => (
				<InfoItem key={nameColor}>
					<CircleColor color={backgroundColor} />
					<Typography variant="s-400">
						<Typography variant="s-600">
							{t(`tagNames.${nameColor}`)}
						</Typography>
						: {t(`tagDescriptions.${descriptionColor}`)}
					</Typography>
				</InfoItem>
			))}
		</div>
	);
}

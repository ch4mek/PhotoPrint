import { useTheme } from '@emotion/react';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { FULL_MONTH_DATE_TYPE } from '@/constants/date';
import dayjs from '@/lib/dayjs';

import { ChatDate, ChatDateLabelWrapper } from './styles';
import { ChatDateLabelProps } from './types';

export function ChatDateLabel({ date, isTheFirstDate }: ChatDateLabelProps) {
	const theme = useTheme();
	const formattedDate = dayjs(date).format(FULL_MONTH_DATE_TYPE);

	return (
		<ChatDateLabelWrapper>
			<ChatDate isTheFirstDate={isTheFirstDate}>
				<Typography variant="xs-400" color={theme.palette['text-gray']}>
					{formattedDate}
				</Typography>
			</ChatDate>
		</ChatDateLabelWrapper>
	);
}

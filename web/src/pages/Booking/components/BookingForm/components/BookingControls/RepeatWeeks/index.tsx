import { Counter } from '@mms/mms-ui-library/components/Counter';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { memo } from 'react';
import { useTranslation } from 'react-i18next';

import {
	REPEAT_WEEKS_MAX_VALUE,
	REPEAT_WEEKS_MIN_VALUE,
} from '@/constants/booking';

import { getWeeksWord } from './constants';
import { Wrapper } from './styles';
import type { RepeatWeeksInnerProps } from './types';

function RepeatWeeksInner({ value, onChange }: RepeatWeeksInnerProps) {
	const { t } = useTranslation('booking', {
		keyPrefix: 'form.repetitive',
	});

	return (
		<Wrapper>
			<Typography variant="m-400">
				{t('repeatEvery', { count: value })}
			</Typography>
			<Counter
				value={value}
				onChange={onChange}
				maxValue={REPEAT_WEEKS_MAX_VALUE}
				minValue={REPEAT_WEEKS_MIN_VALUE}
			/>
			<Typography variant="m-400">{getWeeksWord(value, t)}</Typography>
		</Wrapper>
	);
}

export const RepeatWeeks = memo(RepeatWeeksInner);

import { AfternoonIcon } from '@mms/mms-ui-library/icons/base/AfternoonIcon';
import { EveningIcon } from '@mms/mms-ui-library/icons/base/EveningIcon';
import { MorningIcon } from '@mms/mms-ui-library/icons/base/MorningIcon';

import type { AllowedKeys, GreetingsKey } from '@/i18n/types';

export const UPDATE_GREETING_INTERVAL = 60000;

const greetingKeys = {
	morning: 'goodMorning',
	afternoon: 'goodAfternoon',
	evening: 'goodEvening',
} as const satisfies AllowedKeys<GreetingsKey>;

const { morning, afternoon, evening } = greetingKeys;

export const homeTimesOfDayOptions = {
	morning: { start: 5, end: 12, value: morning, icon: MorningIcon },
	afternoon: {
		start: 12,
		end: 17,
		value: afternoon,
		icon: AfternoonIcon,
	},
	evening: { value: evening, icon: EveningIcon },
} as const;

export const titleStylesConfig = {
	display: 'inline',
	verticalAlign: 'top',
};

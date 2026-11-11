import { useCallback } from 'react';
import { useTranslation } from 'react-i18next';

import {
	localizeTimesheetModeOption,
	localizeTimesheetModeOptions,
} from '../utils/localizeTimesheetModeOption';

type LocalizableOption = {
	id: string | number;
	fullName: string;
	shortName?: string;
};

export const useLocalizedTimesheetModeOptions = () => {
	const { t } = useTranslation('timesheet', { keyPrefix: 'modes' });

	const localizeOption = useCallback(
		<T extends LocalizableOption>(option: T) =>
			localizeTimesheetModeOption(option, t),
		[t]
	);

	const localizeOptions = useCallback(
		<T extends LocalizableOption>(options: T[]) =>
			localizeTimesheetModeOptions(options, t),
		[t]
	);

	return { localizeOption, localizeOptions };
};

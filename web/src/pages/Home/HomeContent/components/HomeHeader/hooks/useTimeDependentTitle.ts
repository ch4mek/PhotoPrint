import { useState, useEffect, useMemo } from 'react';
import { useTranslation } from 'react-i18next';

import { UPDATE_GREETING_INTERVAL } from '../constants';
import { getDayTimeTitle } from '../utils/getDayTimeTitle';

import { useAuth } from '@/context/AuthContext';
import dayjs, { Dayjs } from '@/lib/dayjs';

export const useTimeDependentTitle = () => {
	const [time, setTime] = useState<Dayjs>(dayjs());
	const { t } = useTranslation('home', { keyPrefix: 'greetings' });
	const { firstName } = useAuth();

	const { value, icon } = useMemo(() => {
		const hour = time.hour();

		return getDayTimeTitle(hour);
	}, [time]);

	useEffect(() => {
		const timer = setInterval(() => setTime(dayjs()), UPDATE_GREETING_INTERVAL);

		return () => clearInterval(timer);
	}, []);

	return {
		title: `${t(value)}, ${firstName}!`,
		icon,
	};
};

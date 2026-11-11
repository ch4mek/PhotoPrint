import { createContext } from 'react';

import { NotificationParameteresContextValue } from '../types';

export const NotificationsParameteresContext =
	createContext<NotificationParameteresContextValue>(
		{} as NotificationParameteresContextValue
	);

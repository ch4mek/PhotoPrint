import { useMemo } from 'react';

import { splitSettingsName } from '../utils';

import { GetAdminPortalSettingsResponse } from '@/queries/types';

export const useRows = (data?: GetAdminPortalSettingsResponse[]) =>
	useMemo(
		() =>
			data
				? data.map(({ name, ...rest }) => ({
						name: splitSettingsName(name),
						...rest,
				  }))
				: [],
		[data]
	);

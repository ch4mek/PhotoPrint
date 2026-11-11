import { useEffect } from 'react';
import { useSearchParams } from 'react-router-dom';

import { UseCleanDeniedParamsProps } from './types';

export const useCleanDeniedParams = ({
	deniedPopUps,
	isUserInitialized,
}: UseCleanDeniedParamsProps) => {
	const [, setSearchParams] = useSearchParams();

	useEffect(() => {
		if (!isUserInitialized || !deniedPopUps.length) {
			return;
		}

		setSearchParams((old) => {
			const newParams = new URLSearchParams(old);
			deniedPopUps.forEach((key) => newParams.delete(key));

			return newParams;
		});
	}, [deniedPopUps, isUserInitialized, setSearchParams]);
};

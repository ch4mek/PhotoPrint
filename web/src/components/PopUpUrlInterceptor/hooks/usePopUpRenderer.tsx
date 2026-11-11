import { LinearLoader } from '@mms/mms-ui-library/components/LinearLoader';
import { LoaderVariant } from '@mms/mms-ui-library/components/LinearLoader/types';
import { Suspense } from 'react';
import { useSearchParams } from 'react-router-dom';

import { searchParamsConfig } from '../config';

import { PopUpSearchParams } from '@/constants/searchParams';
import { LoaderWrapper } from '@/layouts/main/styles';

import { UsePopUpRendererProps } from './types';

export const usePopUpRenderer = ({
	allowedPopUps,
	isUserInitialized,
}: UsePopUpRendererProps) => {
	const [, setSearchParams] = useSearchParams();

	if (!isUserInitialized) {
		return null;
	}

	return allowedPopUps.map(([key, value]) => {
		const handlePageClose = () => {
			setSearchParams((old) => {
				const newParams = new URLSearchParams(old);
				newParams.delete(key);

				return newParams;
			});
		};

		const PopUp = searchParamsConfig[key as PopUpSearchParams].PopUp;

		return (
			<Suspense
				fallback={
					<LoaderWrapper>
						<LinearLoader variant={LoaderVariant.Indeterminate} />
					</LoaderWrapper>
				}
				key={key}
			>
				<PopUp handleClose={handlePageClose} id={value} />
			</Suspense>
		);
	});
};

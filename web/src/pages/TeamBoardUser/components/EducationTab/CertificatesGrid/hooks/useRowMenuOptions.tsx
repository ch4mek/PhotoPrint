import { PickerButtonOption } from '@mms/mms-ui-library/components/PickerButton';
import { BasketIcon } from '@mms/mms-ui-library/icons/base/BasketIcon';
import { ViewIcon } from '@mms/mms-ui-library/icons/base/ViewIcon';
import { useContext, useMemo } from 'react';

import { PersonalDataContext } from '@/pages/TeamBoardUser/context/personalDataContext';
import { isFullAccess } from '@/pages/TeamBoardUser/utils';

export const useRowMenuOptions = ({
	handleRemoveCertificate,
	handleViewCertificate,
}: {
	handleRemoveCertificate: VoidFunction;
	handleViewCertificate: VoidFunction;
}) => {
	const { accessLevel } = useContext(PersonalDataContext);

	const certificateMenuOptions = useMemo<PickerButtonOption[]>(() => {
		const canDeleteCertificate = isFullAccess(accessLevel);

		const options: PickerButtonOption[] = [
			{
				value: 'View',
				addon: <ViewIcon />,
				onClick: handleViewCertificate,
			},
		];

		if (canDeleteCertificate) {
			options.push({
				value: 'Delete',
				addon: <BasketIcon />,
				onClick: handleRemoveCertificate,
			});
		}

		return options;
	}, [accessLevel, handleViewCertificate, handleRemoveCertificate]);

	return certificateMenuOptions;
};

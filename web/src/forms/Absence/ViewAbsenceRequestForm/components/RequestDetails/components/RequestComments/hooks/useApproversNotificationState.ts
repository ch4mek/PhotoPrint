import { useState } from 'react';

export const useApproversNotificationState = () => {
	const [isNotifyApproversChecked, setIsNotifyApproversChecked] =
		useState(true);

	const toggleNotifyApproversCheckbox = () =>
		setIsNotifyApproversChecked((prev) => !prev);

	return {
		isNotifyApproversChecked,
		toggleNotifyApproversCheckbox,
	};
};

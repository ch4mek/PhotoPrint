import { useCallback, useEffect, useRef, useState } from 'react';

import { useToast } from '@/context/Toast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';

export function useRowUpdater(
	plannedFte: number,
	handleRowChange?: (value: number) => void
) {
	const [inputState, setInputState] = useState({
		plannedHours: plannedFte.toString(),
		isActive: false,
	});

	const toast = useToast();
	const inputRef = useRef<HTMLInputElement>(null);

	useEffect(() => {
		if (inputState.isActive) {
			inputRef.current?.focus();
		}
	}, [inputState.isActive]);

	const setPlannedHours = (hours: string) =>
		setInputState((prev) => ({ ...prev, plannedHours: hours }));

	const setIsInputActive = (isActive: boolean) =>
		setInputState((prev) => ({ ...prev, isActive }));

	const handlePlannedHoursAccept = useCallback((value: unknown) => {
		if (typeof value === 'string') {
			setPlannedHours(value);
		}
	}, []);

	const updateRowData = useCallback(
		(value: number) => {
			setPlannedHours(String(value));
			handleRowChange?.(value);
			setIsInputActive(false);
		},
		[handleRowChange]
	);

	const acceptChanges = useCallback(
		(newWorkload: number) => updateRowData(newWorkload),
		[updateRowData]
	);

	const handleUpdateError = (value: number) => () => {
		updateRowData(value);
		createOnErrorHandler(toast);
	};

	useEffect(() => {
		setInputState((prev) => ({
			...prev,
			plannedHours: plannedFte.toString(),
		}));
	}, [plannedFte]);

	return {
		inputState,
		inputRef,
		setPlannedHours,
		setIsInputActive,
		handlePlannedHoursAccept,
		acceptChanges,
		handleUpdateError,
	};
}

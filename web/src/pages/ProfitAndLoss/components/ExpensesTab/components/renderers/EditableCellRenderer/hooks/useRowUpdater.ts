import { AxiosError } from 'axios';
import { useCallback, useEffect, useRef, useState } from 'react';

import { useToast } from '@/context/Toast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { ServerResponseError } from '@/queries/types';

export function useRowUpdater(displayValue: string) {
	const [inputState, setInputState] = useState({
		cellValue: displayValue,
		isActive: false,
	});

	const toast = useToast();
	const inputRef = useRef<HTMLInputElement>(null);

	useEffect(() => {
		if (inputState.isActive) {
			inputRef.current?.focus();
		}
	}, [inputState.isActive]);

	const setCellValue = (value: string) =>
		setInputState((prev) => ({ ...prev, cellValue: value }));

	const setIsInputActive = (isActive: boolean) =>
		setInputState((prev) => ({ ...prev, isActive }));

	const handleCellValueAccept = useCallback((value: unknown) => {
		if (typeof value === 'string') {
			setCellValue(value);

			return;
		}

		if (typeof value === 'number' && !Number.isNaN(value)) {
			setCellValue(String(value));

			return;
		}

		if (!value) {
			setCellValue('');
		}
	}, []);

	const acceptChanges = useCallback((newValue: number) => {
		setCellValue(String(newValue));
		setIsInputActive(false);
	}, []);

	const handleUpdateError = useCallback(
		(revertValue: number) => (error: AxiosError<ServerResponseError>) => {
			setCellValue(String(revertValue));
			setIsInputActive(false);
			createOnErrorHandler(toast)(error);
		},
		[toast]
	);

	useEffect(() => {
		setInputState((prev) => ({
			...prev,
			cellValue: displayValue,
		}));
	}, [displayValue]);

	return {
		inputState,
		inputRef,
		setCellValue,
		setIsInputActive,
		handleCellValueAccept,
		acceptChanges,
		handleUpdateError,
	};
}

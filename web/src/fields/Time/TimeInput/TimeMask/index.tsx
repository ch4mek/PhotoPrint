import { ChangeEvent, FocusEvent, useCallback } from 'react';
import { IMaskInput } from 'react-imask';

import { convertStringDateToValidFormat } from '@/helpers/validTimeChecks';

import { maskBlocks } from './constants';
import { TimeMaskInputProps } from './types';

export function TimeMask(props: TimeMaskInputProps) {
	const { onChange, value, onBlurEvent, onFocus, ref, ...other } = props;

	const handleFocus = useCallback(
		(event: FocusEvent<HTMLInputElement>) => {
			event.target.select();

			if (onFocus) {
				onFocus(event);
			}
		},
		[onFocus]
	);

	const handleBlur = useCallback(
		(event: ChangeEvent<HTMLInputElement>) => {
			onChange(convertStringDateToValidFormat(value));

			if (onBlurEvent) {
				onBlurEvent(event);
			}
		},
		[value]
	);

	const handleChange = (val: string) => {
		if (val[2] === ':' && val.length === 3) {
			const newVal = val.slice(0, 2);
			onChange(newVal);
		} else {
			onChange(val);
		}
	};

	return (
		<IMaskInput
			{...other}
			inputRef={ref}
			autoComplete="off"
			value={value}
			mask="HH:MM"
			onAccept={handleChange}
			onFocus={handleFocus}
			onBlur={handleBlur}
			blocks={maskBlocks}
		/>
	);
}

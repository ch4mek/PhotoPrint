import { ChangeEvent, useCallback } from 'react';
import { IMaskInput } from 'react-imask';

import { maskBlocks } from './constants';
import { HoursInputWrapper } from './styles';
import { TimeMaskInputProps } from './types';

export function HoursInput(props: TimeMaskInputProps) {
	const {
		onChange,
		value,
		onBlurEvent,
		disabled,
		onFocusEvent,
		hasError,
		...other
	} = props;

	const handleFocus = useCallback(
		(event: ChangeEvent<HTMLInputElement>) => {
			if (onFocusEvent) {
				onFocusEvent(event);
			}
			event.target.select();
		},
		[onFocusEvent]
	);
	const handleBlur = useCallback(
		(event: ChangeEvent<HTMLInputElement>) => {
			if (disabled) {
				return;
			}

			if (onBlurEvent) {
				onBlurEvent(event);
			}
		},
		[value, disabled]
	);

	return (
		<HoursInputWrapper disabled={disabled} hasError={hasError}>
			<IMaskInput
				{...other}
				disabled={disabled}
				autoComplete="off"
				value={value}
				mask="HHHH"
				onAccept={onChange}
				onFocus={handleFocus}
				onBlur={handleBlur}
				blocks={maskBlocks}
			/>
		</HoursInputWrapper>
	);
}

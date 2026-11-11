import { useCallback } from 'react';

import { EVENT_ENTER_KEY } from '@/constants/eventKeys';
import { TimeMask } from '@/fields/Time/TimeInput/TimeMask';

import { InputWrapper } from './styles';
import { CellInputProps } from './types';

export function CellInput({
	onChange,
	value,
	onBlur,
	disabled = false,
	onFocus,
	cellPosition,
	ref,
}: CellInputProps) {
	const handleClick = useCallback((e: React.MouseEvent<HTMLInputElement>) => {
		e.stopPropagation();
		e.preventDefault();
	}, []);

	const handleKeyDown = useCallback(
		(e: React.KeyboardEvent<HTMLDivElement>) => {
			e.stopPropagation();
		},
		[]
	);

	const handleInputKeyDown = useCallback(
		(e: React.KeyboardEvent<HTMLInputElement>) => {
			if (e.key === EVENT_ENTER_KEY) {
				e.currentTarget.blur();
			}
		},
		[]
	);

	return (
		<InputWrapper
			disabled={disabled}
			onKeyDown={handleKeyDown}
			onBlur={onBlur}
			cellPosition={cellPosition}
		>
			<TimeMask
				ref={ref}
				disabled={disabled}
				onChange={onChange}
				onFocus={onFocus}
				onKeyDown={handleInputKeyDown}
				value={value}
				onClick={handleClick}
			/>
		</InputWrapper>
	);
}

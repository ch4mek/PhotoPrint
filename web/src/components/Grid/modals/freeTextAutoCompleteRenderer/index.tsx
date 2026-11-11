import { useTheme } from '@emotion/react';
import { Input } from '@mms/mms-ui-library/components/Input';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { ChangeEvent, useMemo, MouseEvent, useCallback } from 'react';

import { ControlButtons } from '../../components';
import { useAutoFocusInput } from '../../hooks/useAutoFocusInput';
import { RenderModalProps } from '../../renderers';

import { NO_RESULTS_FOUND } from '@/constants/errors';

import {
	CustomScroll,
	ModalWrapper,
	ScrollbarWrapper,
	TextOverflow,
	TypographyNoResult,
	TypographyWrapper,
} from './styles';

export function FreeTextAutoComplete({
	onApply,
	onReset,
	selection,
	setSelection,
	applyDisabled,
	resetDisabled,
	autocompleteOptions = [],
	isOpen = false,
}: RenderModalProps<string>) {
	const inputRef = useAutoFocusInput(isOpen);
	const theme = useTheme();

	const handleChangeSearchValue = useCallback(
		(e: ChangeEvent<HTMLInputElement>) => {
			e.stopPropagation();

			setSelection(e.target.value);
		},
		[]
	);

	const handleSelectOption =
		(option: string) => (e: MouseEvent<HTMLDivElement>) => {
			e.stopPropagation();

			setSelection(option);
		};

	const filteredOptionsByEnteredValue = useMemo(() => {
		const isFound = autocompleteOptions.includes(selection);

		if (isFound) {
			return [
				selection,
				...autocompleteOptions.filter((option) => option !== selection),
			];
		}

		return autocompleteOptions.filter((option) =>
			option.toLowerCase().includes(selection.toLowerCase())
		);
	}, [autocompleteOptions, selection]);

	return (
		<ModalWrapper>
			<Input
				size="fullWidth"
				value={selection}
				onChange={handleChangeSearchValue}
				autoComplete="off"
				placeholder="Search"
				ref={inputRef}
			/>
			<ScrollbarWrapper>
				{filteredOptionsByEnteredValue.length >= 1 ? (
					<CustomScroll maxWidth="100%" maxHeight="280px" scroll="horizontal">
						{filteredOptionsByEnteredValue.map((option, index) => (
							<TypographyWrapper
								key={index}
								onClick={handleSelectOption(option)}
								tabIndex={0}
								isActive={option === selection}
							>
								<Typography
									color={theme.palette['text-black']}
									countOfLines={1}
									variant="m-400"
								>
									<Tooltip useIfTextCut text={option}>
										<TextOverflow>{option}</TextOverflow>
									</Tooltip>
								</Typography>
							</TypographyWrapper>
						))}
					</CustomScroll>
				) : (
					<TypographyNoResult variant="m-400">
						{NO_RESULTS_FOUND}
					</TypographyNoResult>
				)}
			</ScrollbarWrapper>

			<ControlButtons
				onApply={onApply}
				onReset={onReset}
				applyDisabled={applyDisabled}
				resetDisabled={resetDisabled}
			/>
		</ModalWrapper>
	);
}

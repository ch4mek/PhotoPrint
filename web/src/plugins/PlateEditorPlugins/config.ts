import {
	AutoformatPlugin,
	createPlateUI,
	DOMHandler,
	ELEMENT_BLOCKQUOTE,
	ELEMENT_CODE_BLOCK,
	ELEMENT_H1,
	ELEMENT_H2,
	ELEMENT_H3,
	ELEMENT_H4,
	ELEMENT_H5,
	ELEMENT_H6,
	ELEMENT_HR,
	ELEMENT_IMAGE,
	ELEMENT_PARAGRAPH,
	ELEMENT_TD,
	ELEMENT_TODO_LI,
	ExitBreakPlugin,
	IndentPlugin,
	isBlockAboveEmpty,
	isSelectionAtBlockStart,
	KEYS_HEADING,
	PlateEditor,
	ResetNodePlugin,
	SelectOnBackspacePlugin,
	SoftBreakPlugin,
	TEditableProps,
	TrailingBlockPlugin,
} from '@udecode/plate';
import { KeyboardEvent } from 'react';

import { DEFAULT_PLATE_EDITOR_PLACEHOLDER } from '@/constants/plateEditor';
import { autoformatRules } from '@/pages/Wiki/autoformat/autoformatRules';
import {
	CustomPlatePlugin,
	CustomValue,
	CustomEditor,
} from '@/types/plateEditor';

const resetBlockTypesCommonRule = {
	types: [ELEMENT_BLOCKQUOTE, ELEMENT_TODO_LI],
	defaultType: ELEMENT_PARAGRAPH,
};

const COMMON_VALID_TYPES = [
	ELEMENT_PARAGRAPH,
	ELEMENT_H1,
	ELEMENT_H2,
	ELEMENT_H3,
	ELEMENT_H4,
	ELEMENT_H5,
	ELEMENT_H6,
];

export const CONFIG: {
	components: Record<string, any>;
	editableProps: TEditableProps<CustomValue>;
	align: Partial<CustomPlatePlugin>;
	autoformat: Partial<
		CustomPlatePlugin<AutoformatPlugin<CustomValue, CustomEditor>>
	>;
	exitBreak: Partial<CustomPlatePlugin<ExitBreakPlugin>>;
	indent: Partial<CustomPlatePlugin<IndentPlugin>>;
	lineHeight: Partial<CustomPlatePlugin>;
	resetBlockType: Partial<CustomPlatePlugin<ResetNodePlugin>>;
	selectOnBackspace: Partial<CustomPlatePlugin<SelectOnBackspacePlugin>>;
	softBreak: Partial<CustomPlatePlugin<SoftBreakPlugin>>;
	trailingBlock: Partial<CustomPlatePlugin<TrailingBlockPlugin>>;
} = {
	editableProps: {
		spellCheck: false,
		autoFocus: false,
		placeholder: DEFAULT_PLATE_EDITOR_PLACEHOLDER,
		style: {
			padding: 0,
		},
	},
	components: createPlateUI(),

	align: {
		inject: {
			props: {
				validTypes: [...COMMON_VALID_TYPES],
			},
		},
	},
	indent: {
		inject: {
			props: {
				validTypes: [
					...COMMON_VALID_TYPES,
					ELEMENT_BLOCKQUOTE,
					ELEMENT_CODE_BLOCK,
				],
			},
		},
		handlers: {
			onKeyDown: ((event: KeyboardEvent<Element>) => {
				if (event.key === 'Tab') {
					event.preventDefault();
					event.stopPropagation();

					return false;
				}

				return undefined;
			}) as unknown as DOMHandler<
				IndentPlugin,
				CustomValue,
				PlateEditor<CustomValue>,
				KeyboardEvent
			>,
		},
	},
	lineHeight: {
		inject: {
			props: {
				defaultNodeValue: 1.5,
				validNodeValues: [1, 1.2, 1.5, 2, 3],
				validTypes: [...COMMON_VALID_TYPES],
			},
		},
	},
	resetBlockType: {
		options: {
			rules: [
				{
					...resetBlockTypesCommonRule,
					hotkey: 'Enter',
					predicate: isBlockAboveEmpty,
				},
				{
					...resetBlockTypesCommonRule,
					hotkey: 'Backspace',
					predicate: isSelectionAtBlockStart,
				},
			],
		},
	},
	trailingBlock: { type: ELEMENT_PARAGRAPH },
	softBreak: {
		options: {
			rules: [
				{ hotkey: 'shift+enter' },
				{
					hotkey: 'enter',
					query: {
						allow: [ELEMENT_CODE_BLOCK, ELEMENT_BLOCKQUOTE, ELEMENT_TD],
					},
				},
			],
		},
	},
	exitBreak: {
		options: {
			rules: [
				{
					hotkey: 'mod+enter',
				},
				{
					hotkey: 'mod+shift+enter',
					before: true,
				},
				{
					hotkey: 'enter',
					query: {
						start: true,
						end: true,
						allow: KEYS_HEADING,
					},
				},
			],
		},
	},
	selectOnBackspace: {
		options: {
			query: {
				allow: [ELEMENT_IMAGE, ELEMENT_HR],
			},
		},
	},
	autoformat: {
		options: {
			rules: autoformatRules,
		},
	},
};

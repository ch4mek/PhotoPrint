/* eslint-disable no-param-reassign */
import {
	PlateEditor,
	toggleMark,
	toggleList,
	LinkToolbarButton,
	getMarks,
	focusEditor,
} from '@udecode/plate';
import {
	ReactNode,
	MouseEvent as ReactMouseEvent,
	useState,
	useEffect,
} from 'react';

import { TOOLBAR_BUTTON_CLASSNAME } from '@/components/PlateToolbars/constants';
import { useToolbarPopUp } from '@/components/PlateToolbars/hooks/useToolbarPopUp';
import { CustomValue, MediaType } from '@/types/plateEditor';

import { ButtonWrapper } from './styled';

export function WithAlwaysVisibleMarkButton(type: string, Icon: ReactNode) {
	return function AlwaysVisibleMarkButton({
		editor,
		className,
	}: {
		editor: PlateEditor<CustomValue>;
		className: string;
	}) {
		const [isActive, setIsActive] = useState(false);

		useEffect(() => {
			const updateActiveState = () => {
				const marks = getMarks(editor);
				setIsActive(!!marks?.[type]);
			};

			updateActiveState();

			const { onChange } = editor;
			editor.onChange = () => {
				onChange();
				updateActiveState();
			};

			return () => {
				editor.onChange = onChange;
			};
		}, [editor, type]);

		const handleMouseDown = (e: ReactMouseEvent) => {
			e.preventDefault();

			toggleMark(editor, { key: type });

			const marks = getMarks(editor);
			setIsActive(!!marks?.[type]);

			setTimeout(() => focusEditor(editor), 0);
		};

		return (
			<ButtonWrapper
				className={className}
				onMouseDown={handleMouseDown}
				type="button"
				view="outline"
				isActive={isActive}
			>
				{Icon}
			</ButtonWrapper>
		);
	};
}

export function WithAlwaysVisibleListButton(type: string, Icon: ReactNode) {
	return function AlwaysVisibleListButton({
		editor,
		className,
	}: {
		editor: PlateEditor<CustomValue>;
		className: string;
	}) {
		const handleMouseDown = (e: ReactMouseEvent) => {
			e.preventDefault();
			toggleList(editor, { type });
		};

		return (
			<ButtonWrapper
				className={className}
				onMouseDown={handleMouseDown}
				type="button"
				view="outline"
			>
				{Icon}
			</ButtonWrapper>
		);
	};
}

export function WithAlwaysVisibleLinkButton(
	Icon: ReactNode,
	handleSubmit: (data: { url: string }) => void
) {
	return function AlwaysVisibleLinkButton() {
		const { handleButtonClick } = useToolbarPopUp();

		return (
			<LinkToolbarButton
				icon={Icon}
				onMouseDown={handleButtonClick(MediaType.Link, handleSubmit)}
				classNames={{ root: TOOLBAR_BUTTON_CLASSNAME, active: '' }}
			/>
		);
	};
}

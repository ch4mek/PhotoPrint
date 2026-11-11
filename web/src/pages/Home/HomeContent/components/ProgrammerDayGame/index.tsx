import { useEffect, useRef, useState } from 'react';

import { useAuth } from '@/context/AuthContext';
import { useSaveProgrammerDayScore } from '@/queries/home/useSaveProgrammerDayScore';

import {
	BEST_SCORE_MESSAGE_TYPE,
	GAME_FRAME_HEIGHT_BUFFER,
	GAME_PATH,
	INITIAL_GAME_FRAME_HEIGHT,
} from './constants';
import { ProgrammerDayGameFrame, ProgrammerDayGameWrapper } from './styles';
import { ProgrammerDayGameMessage, ProgrammerDayGameProps } from './types';

export function ProgrammerDayGame({ onBestScore }: ProgrammerDayGameProps) {
	const { id: userId } = useAuth();
	const frameRef = useRef<HTMLIFrameElement>(null);
	const resizeObserverRef = useRef<ResizeObserver | null>(null);
	const [frameHeight, setFrameHeight] = useState(INITIAL_GAME_FRAME_HEIGHT);
	const { mutate: saveProgrammerDayScore } = useSaveProgrammerDayScore(userId);

	const handleFrameLoad = () => {
		const gameContent =
			frameRef.current?.contentDocument?.querySelector<HTMLElement>('.wrap');

		if (!gameContent) {
			return;
		}

		const updateFrameHeight = () =>
			setFrameHeight(
				Math.ceil(gameContent.getBoundingClientRect().height) +
					GAME_FRAME_HEIGHT_BUFFER
			);

		resizeObserverRef.current?.disconnect();
		resizeObserverRef.current = new ResizeObserver(updateFrameHeight);
		resizeObserverRef.current.observe(gameContent);
		updateFrameHeight();
	};

	useEffect(
		() => () => {
			resizeObserverRef.current?.disconnect();
		},
		[]
	);

	useEffect(() => {
		const forwardSpaceToGame = (event: KeyboardEvent) => {
			const isInteractiveTarget =
				event.target instanceof Element &&
				event.target.closest(
					'input, textarea, select, button, a, [contenteditable="true"]'
				);

			if (event.code !== 'Space' || isInteractiveTarget) {
				return;
			}

			event.preventDefault();
			frameRef.current?.contentWindow?.dispatchEvent(
				new KeyboardEvent(event.type, {
					bubbles: true,
					cancelable: true,
					code: event.code,
					key: event.key,
					repeat: event.repeat,
				})
			);
		};

		window.addEventListener('keydown', forwardSpaceToGame);
		window.addEventListener('keyup', forwardSpaceToGame);

		return () => {
			window.removeEventListener('keydown', forwardSpaceToGame);
			window.removeEventListener('keyup', forwardSpaceToGame);
		};
	}, []);

	useEffect(() => {
		const handleMessage = (event: MessageEvent<ProgrammerDayGameMessage>) => {
			const isGameMessage =
				event.origin === window.location.origin &&
				event.source === frameRef.current?.contentWindow &&
				event.data?.type === BEST_SCORE_MESSAGE_TYPE;
			const score = event.data?.score;

			if (
				isGameMessage &&
				typeof score === 'number' &&
				Number.isInteger(score) &&
				score >= 0
			) {
				saveProgrammerDayScore({ score });
				onBestScore?.(score);
			}
		};

		window.addEventListener('message', handleMessage);

		return () => window.removeEventListener('message', handleMessage);
	}, [onBestScore, saveProgrammerDayScore]);

	return (
		<ProgrammerDayGameWrapper>
			<ProgrammerDayGameFrame
				ref={frameRef}
				height={frameHeight}
				onLoad={handleFrameLoad}
				src={GAME_PATH}
				title="Игра ко Дню программиста"
			/>
		</ProgrammerDayGameWrapper>
	);
}

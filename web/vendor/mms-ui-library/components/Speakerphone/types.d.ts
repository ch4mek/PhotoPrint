import { CSSProperties, RefObject } from 'react';
export type SpeechRecognitionController = Partial<{
    continuous: boolean;
    interimResults: boolean;
    lang: string;
    maxAlternatives: number;
    onaudioend: (event: SpeechRecognitionController) => void;
    onaudiostart: (event: SpeechRecognitionController) => void;
    onnomatch: (event: SpeechRecognitionController) => void;
    onsoundend: (event: SpeechRecognitionController) => void;
    onsoundstart: (event: SpeechRecognitionController) => void;
    onspeechend: (event: SpeechRecognitionController) => void;
    onspeechstart: (event: SpeechRecognitionController) => void;
    onstart: (event: SpeechRecognitionController) => void;
    onresult: (event: {
        results: SpeechRecognitionResultList;
    }) => void;
    onerror: (event: SpeechRecognitionController) => void;
    onend: (event: SpeechRecognitionController) => void;
    abort: VoidFunction;
    start: VoidFunction;
    stop: VoidFunction;
}>;
export interface SpeakerphoneStylesConfig {
    activeSpeakerIconWrapper?: CSSProperties;
}
export interface SpeechRecognitionProps {
    onChange: (value: string) => void;
    inputRef: RefObject<HTMLDivElement | null>;
    disabled?: boolean;
    stylesConfig?: SpeakerphoneStylesConfig;
}
export interface FillSpeakerphoneIconWrapperProps {
    extraStyles?: CSSProperties;
}

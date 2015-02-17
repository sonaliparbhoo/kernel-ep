﻿/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.       *
*                                                       *
********************************************************/

namespace MicrosoftResearch.Infer.Learners.BayesPointMachineClassifierInternal
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    using MicrosoftResearch.Infer.Distributions;
    using MicrosoftResearch.Infer.Learners.Mappings;
    using MicrosoftResearch.Infer.Maths;

    /// <summary>
    /// An abstract base class for a multi-class Bayes point machine classifier which operates on data in the standard format.
    /// </summary>
    /// <typeparam name="TInstanceSource">The type of the instance source.</typeparam>
    /// <typeparam name="TInstance">The type of an instance.</typeparam>
    /// <typeparam name="TLabelSource">The type of the label source.</typeparam>
    /// <typeparam name="TLabel">The type of a label.</typeparam>
    /// <typeparam name="TTrainingSettings">The type of the settings for training.</typeparam>
    [Serializable]
    internal abstract class MulticlassStandardDataFormatBayesPointMachineClassifier<TInstanceSource, TInstance, TLabelSource, TLabel, TTrainingSettings> :
        StandardDataFormatBayesPointMachineClassifier<TInstanceSource, TInstance, TLabelSource, TLabel, int, Discrete, TTrainingSettings, MulticlassBayesPointMachineClassifierPredictionSettings<TLabel>, MulticlassBayesPointMachineClassifierPredictionSettings<int>>
        where TTrainingSettings : BayesPointMachineClassifierTrainingSettings
    {
        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="MulticlassStandardDataFormatBayesPointMachineClassifier{TInstanceSource,TInstance,TLabelSource,TLabel,TTrainingSettings}"/> class.
        /// </summary>
        /// <param name="standardMapping">The mapping used for accessing data in the standard format.</param>
        protected MulticlassStandardDataFormatBayesPointMachineClassifier(IClassifierMapping<TInstanceSource, TInstance, TLabelSource, TLabel, Vector> standardMapping)
        {
            Debug.Assert(standardMapping != null, "The mapping must not be null.");

            this.NativeMapping = new MulticlassNativeClassifierMapping<TInstanceSource, TInstance, TLabelSource, TLabel>(standardMapping);
        }

        /// <summary>
        /// Gets the data mapping into the native format.
        /// </summary>
        protected MulticlassNativeClassifierMapping<TInstanceSource, TInstance, TLabelSource, TLabel> NativeMapping { get; private set; }

        /// <summary>
        /// Sets the number of batches the training data is split into and resets constraints on the weight distributions.
        /// </summary>
        /// <param name="value">The number of batches to use.</param>
        protected override void SetBatchCount(int value)
        {
            this.NativeMapping.SetBatchCount(value);
        }

        /// <summary>
        /// Gets the distribution over class labels in the standard data format.
        /// </summary>
        /// <param name="nativeLabelDistribution">The distribution over class labels in the native data format.</param>
        /// <returns>The class label distribution in standard data format.</returns>
        protected override IDictionary<TLabel, double> GetStandardLabelDistribution(Discrete nativeLabelDistribution)
        {
            Debug.Assert(nativeLabelDistribution != null, "The distribution over class label identifiers must not be null.");
            Debug.Assert(nativeLabelDistribution.Dimension > 1, "There must be at least two distinct class label identifiers.");
            Debug.Assert(
                this.NativeMapping.GetClassCount() == nativeLabelDistribution.Dimension,
                "The number of class labels and class label identifiers must be equal.");

            var labelDistribution = new Dictionary<TLabel, double>();
            for (int i = 0; i < nativeLabelDistribution.Dimension; i++)
            {
                labelDistribution.Add(this.GetStandardLabel(i), nativeLabelDistribution[i]);                    
            }

            return labelDistribution;
        }

        /// <summary>
        /// Gets the class labels in the standard data format.
        /// </summary>
        /// <param name="nativeLabel">The class labels in the native data format.</param>
        /// <returns>The class label in standard data format.</returns>
        protected override TLabel GetStandardLabel(int nativeLabel)
        {
            return this.NativeMapping.GetStandardLabel(nativeLabel);
        }

        /// <summary>
        /// Sets the class labels.
        /// </summary>
        /// <param name="instanceSource">The instance source.</param>
        /// <param name="labelSource">An optional label source.</param>
        protected override void SetClassLabels(TInstanceSource instanceSource, TLabelSource labelSource = default(TLabelSource))
        {
            this.NativeMapping.SetClassLabels(instanceSource, labelSource);
        }

        /// <summary>
        /// Checks that the class labels provided by the mapping are consistent.
        /// </summary>
        /// <param name="instanceSource">The instance source.</param>
        /// <param name="labelSource">An optional label source.</param>
        protected override void CheckClassLabelConsistency(TInstanceSource instanceSource, TLabelSource labelSource = default(TLabelSource))
        {
            if (!this.NativeMapping.CheckClassLabelConsistency(instanceSource, labelSource))
            {
                throw new BayesPointMachineClassifierException("The class labels must not change.");
            }
        }

        /// <summary>
        /// Updates the prediction settings of the native data format classifier with those of the standard data format classifier.
        /// </summary>
        protected override void UpdateNativePredictionSettings()
        {
            this.Classifier.Settings.Prediction.IterationCount = this.Settings.Prediction.IterationCount;
        }
    }
}